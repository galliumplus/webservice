import sys
import json
import os
import re


def main():
    input_file = sys.argv[1]
    with open(input_file, "rt") as f:
        logs_data = json.load(f)

    print(f"{len(logs_data)} lignes chargées")

    mappings = {}
    mappings_file = sys.argv[1] + ".mappings"
    if os.path.exists(mappings_file):
        with open(mappings_file, "rt") as f:
            mappings = json.load(f)

    schema_file = "log-schema.json"
    with open(schema_file, "rt") as f:
        schema = json.load(f)

    output_file = sys.argv[2]
    counter = 0
    last_id = -1
    with open(output_file, "wt+") as f:
        for log_line in logs_data:
            current_id = log_line["log_id"]
            if current_id <= last_id:
                raise ValueError(
                    f"les identifiants doivent être monotones. {current_id} est apparu après {last_id}"
                )

            mapped = map_line(log_line, mappings)
            for mapped_line in mapped:
                check_line_schema(mapped_line, schema)
                write_line_tsv(f, mapped_line)
            last_id = current_id

            counter += 1
            print(f"\r{counter} lignes traitées", end="")

    if mappings.get("!"):
        del mappings["!"]
        with open(mappings_file, "wt+") as f:
            mappings = json.dump(mappings, f)
        print("\nATTENTION valeurs manquantes dans la table de correspondance")
    else:
        print("\nOK terminé")


def check_line_schema(mapped_line, schema):
    hexcode = hex(mapped_line["action"])[2:].upper()
    if hexcode not in schema:
        raise ValueError(
            f"aucun schéma de log défini pour les actions de type {hexcode}"
        )

    unexpected = set(mapped_line["details"])
    for expected_property in schema[hexcode]:
        if expected_property not in mapped_line["details"]:
            raise ValueError(
                f"les actions de type {hexcode} doivent renseigner un {expected_property}"
            )

        unexpected.remove(expected_property)

        value = mapped_line["details"][expected_property]
        expected_type = schema[hexcode][expected_property]
        if expected_type == "string":
            if not isinstance(value, str):
                raise ValueError(
                    f"le {expected_property} des actions de type {hexcode} doivent être des string, pas {type(value)}"
                )
        elif expected_type == "int":
            if not isinstance(value, int):
                raise ValueError(
                    f"le {expected_property} des actions de type {hexcode} doivent être des int, pas {type(value)}"
                )
        elif expected_type == "number":
            if not isinstance(value, int) and not isinstance(value, float):
                raise ValueError(
                    f"le {expected_property} des actions de type {hexcode} doivent être des number, pas {type(value)}"
                )
        else:
            raise ValueError(f"type de données {expected_type} non pris en charge")

    if unexpected:
        raise ValueError(
            f"les actions de type {hexcode} n'acceptent pas les champs {', '.join(unexpected)}"
        )


def map_line(old_log, mappings):
    mapped = None
    for mapper in [
        connexion,
        compte_creation,
        compte_modification,
        compte_suppression,
        acompte_creation,
        acompte_creation_alt,
        acompte_suppression,
        acompte_suppression_alt,
        stock_ajout_produit,
        stock_ajout_categorie,
        stock_suppression_categorie,
        stock_modification_produit,
        stock_ajout,
        stock_retrait,
        temp_ignored,
        temp_credit_acompte,
        temp_debit_acompte,
        temp2_ignored,
        stock_modification_categorie,
        vente_ignored,
        acompte_credit,
        acompte_debit,
    ]:
        mapped = mapper(old_log, mappings)
        if mapped is not None:
            if isinstance(mapped, list):
                return mapped
            else:
                return [mapped]

    raise ValueError(
        f"aucun convertisseur ne peut prendre en charge la ligne suivante : {old_log}"
    )


def map_client(mappings):
    client_id = mappings.get("client_id")
    if client_id is None:
        mappings["!"] = True
        mappings["client_id"] = None
        return "<missing client mapping>"

    return client_id


def map_user_name(mappings, old_user_name):
    users = mappings.get("users_by_name")
    if users is None:
        mappings["!"] = True
        mappings["users_by_name"] = users = {}

    user_id = users.get(old_user_name)
    if user_id is None:
        mappings["!"] = True
        users[old_user_name] = None
        return "<missing user mapping>"

    return user_id


def map_user_id(mappings, old_user_id):
    users = mappings.get("users_by_id")
    if users is None:
        mappings["!"] = True
        mappings["users_by_id"] = users = {}

    user_id = users.get(old_user_id)
    if user_id is None:
        mappings["!"] = True
        users[old_user_id] = None
        return "<missing user mapping>"

    return user_id


def map_item(mappings, old_item):
    items = mappings.get("items")
    if items is None:
        mappings["!"] = True
        mappings["items"] = items = {}

    item = items.get(old_item)
    if item is None:
        mappings["!"] = True
        items[old_item] = None
        return -1

    return item


def map_category(mappings, old_category):
    categories = mappings.get("categories")
    if categories is None:
        mappings["!"] = True
        mappings["categories"] = categories = {}

    category = categories.get(old_category)
    if category is None:
        mappings["!"] = True
        categories[old_category] = None
        return -1

    return category


def connexion(old_log, mappings):
    if old_log["log_category_id"] != "connexion":
        return None

    match = re.fullmatch(r"Connexion de (.+)", old_log["text"])
    if match is None:
        return None

    if match.group(1) != old_log["user"]:
        return None

    return {
        "action": 0x411,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {},
    }


def compte_creation(old_log, mappings):
    if old_log["log_category_id"] != "compte":
        return None

    match = re.fullmatch(r"Création du compte de (.+)", old_log["text"])
    if match is None:
        match = re.fullmatch(r"Création du compte : (.+)", old_log["text"])
    if match is None:
        return None

    return {
        "action": 0x1B1,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {"id": map_user_name(mappings, match.group(1))},
    }


def compte_modification(old_log, mappings):
    if old_log["log_category_id"] != "compte":
        return None

    match = re.fullmatch(r"Modification d'un compte de (.+)", old_log["text"])
    if match is None:
        match = re.fullmatch(r"Modification du compte : (.+)", old_log["text"])
    if match is None:
        return None

    return {
        "action": 0x1B2,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {"id": map_user_name(mappings, match.group(1))},
    }


def compte_suppression(old_log, mappings):
    if old_log["log_category_id"] != "compte":
        return None

    match = re.fullmatch(r"Suppression du compte de (.+)", old_log["text"])
    if match is None:
        return None

    return {
        "action": 0x1B3,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {"id": map_user_name(mappings, match.group(1))},
    }


def acompte_creation(old_log, mappings):
    if old_log["log_category_id"] != "acompte":
        return None

    match = re.fullmatch(
        r"Création de l'acompte de ([a-z0-9]{8,10}) Avec un montant de ([0-9]+([.,][0-9]+)?)€",
        old_log["text"],
    )
    if match is None:
        return None

    return [
        {
            "action": 0x1B4,
            "time": old_log["date_at"],
            "client": map_client(mappings),
            "user": map_user_name(mappings, old_log["user"]),
            "details": {"id": map_user_id(mappings, match.group(1))},
        },
        {
            "action": 0x311,
            "time": old_log["date_at"],
            "client": map_client(mappings),
            "user": map_user_name(mappings, old_log["user"]),
            "details": {
                "id": map_user_id(mappings, match.group(1)),
                "amount": float(match.group(2).replace(",", ".")),
            },
        },
    ]


def acompte_creation_alt(old_log, mappings):
    if old_log["log_category_id"] != "acompte":
        return None

    match = re.fullmatch(r"Création de l acompte : (.+)", old_log["text"])
    if match is None:
        return None

    return {
        "action": 0x1B4,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {"id": map_user_name(mappings, match.group(1))},
    }


def acompte_suppression(old_log, mappings):
    if old_log["log_category_id"] != "acompte":
        return None

    match = re.fullmatch(
        r"Suppression de l'acompte de ([a-z0-9]{8,10})",
        old_log["text"],
    )
    if match is None:
        return None

    return {
        "action": 0x1B5,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {"id": map_user_id(mappings, match.group(1))},
    }


def acompte_suppression_alt(old_log, mappings):
    if old_log["log_category_id"] != "acompte":
        return None

    match = re.fullmatch(
        r"Suppresion de l acompte : (.+)",
        old_log["text"],
    )
    if match is None:
        return None

    return {
        "action": 0x1B5,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {"id": map_user_name(mappings, match.group(1))},
    }


def stock_modification_produit(old_log, mappings):
    if old_log["log_category_id"] != "stock":
        return None

    match = re.fullmatch(r"Modification d'un produit du Stock\((.+)\)", old_log["text"])
    if match is None:
        return None

    return {
        "action": 0x142,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {"id": map_item(mappings, match.group(1)), "name": match.group(1)},
    }


def stock_ajout_produit(old_log, mappings):
    if old_log["log_category_id"] != "stock":
        return None

    match = re.fullmatch(
        r"Ajout d'un nouveau produit au stock \((.+)\)", old_log["text"]
    )
    if match is None:
        match = re.fullmatch(r"Ajout du produit : (.+)", old_log["text"])
    if match is None:
        return None

    return {
        "action": 0x141,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {"id": map_item(mappings, match.group(1)), "name": match.group(1)},
    }


def stock_ajout_categorie(old_log, mappings):
    if old_log["log_category_id"] != "stock":
        return None

    match = re.fullmatch(r"Ajout de la catégorie (.+) dans le stock", old_log["text"])
    if match is None:
        return None

    return {
        "action": 0x111,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_category(mappings, match.group(1)),
            "name": match.group(1),
        },
    }


def stock_modification_categorie(old_log, mappings):
    if old_log["log_category_id"] != "stock":
        return None

    match = re.fullmatch(
        r"Modification d'une categorie du Stock\((.+)\)", old_log["text"]
    )
    if match is None:
        return None

    return {
        "action": 0x112,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_category(mappings, match.group(1)),
            "name": match.group(1),
        },
    }


def stock_suppression_categorie(old_log, mappings):
    if old_log["log_category_id"] != "stock":
        return None

    match = re.fullmatch(
        r"Suppression de la catégorie : (.+) du stock", old_log["text"]
    )
    if match is None:
        return None

    return {
        "action": 0x113,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_category(mappings, match.group(1)),
            "name": match.group(1),
        },
    }


def stock_ajout(old_log, mappings):
    if old_log["log_category_id"] != "stock":
        return None

    match = re.fullmatch(
        r"Ajout de Stock pour (?P<n>.+) Quantité : (?P<q>[0-9]+)", old_log["text"]
    )
    if match is None:
        match = re.fullmatch(
            r"Ajout du stock \( \+(?P<q>[0-9]*) (?P<n>.*) \)", old_log["text"]
        )
    if match is None:
        return None

    return {
        "action": 0x2F1,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_item(mappings, match.group("n")),
            "quantity": int(match.group("q")),
        },
    }


def stock_retrait(old_log, mappings):
    if old_log["log_category_id"] != "stock":
        return None

    match = re.fullmatch(
        r"Prélèvement d'un produit du Stock\(([0-9]*) (.*)\)", old_log["text"]
    )
    if match is None:
        match = re.fullmatch(
            r"Prélèvement du stock  \( -([0-9]*) (.*) \)", old_log["text"]
        )
    if match is None:
        return None

    if match.group(1) == "":
        return []

    return {
        "action": 0x2F3,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_item(mappings, match.group(2)),
            "quantity": int(match.group(1)),
        },
    }


def temp_ignored(old_log, mappings):
    if old_log["log_category_id"] != "temp":
        return None

    should_ignore = False
    for ignored_re in [
        r"Ajout d'argent sur le compte .+ de [0-9,E+]+ €",
        r"Prélèvement d'argent sur le compte .+ de [0-9,E+]+ €",
        r"Tranversement de € de .+ vers le compte .+",
    ]:
        match = re.fullmatch(ignored_re, old_log["text"])
        should_ignore = should_ignore or match is not None

    if should_ignore:
        return []


def temp_credit_acompte(old_log, mappings):
    if old_log["log_category_id"] != "temp":
        return None

    match = re.fullmatch(
        r"Ajout de ([0-9]+([.,][0-9]*)?) € sur le compte de ([a-z0-9]{7,10})",
        old_log["text"],
    )
    if match is None:
        return None

    return {
        "action": 0x311,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_user_id(mappings, match.group(3)),
            "amount": float(match.group(1).replace(",", ".")),
        },
    }


def acompte_credit(old_log, mappings):
    if old_log["log_category_id"] != "acompte":
        return None

    match = re.fullmatch(
        r"Ajout de ([0-9]+([.,][0-9]*)?) € sur ([a-z0-9]{7,10})",
        old_log["text"],
    )
    if match is None:
        return None

    return {
        "action": 0x311,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_user_id(mappings, match.group(3)),
            "amount": float(match.group(1).replace(",", ".")),
        },
    }


def temp_debit_acompte(old_log, mappings):
    if old_log["log_category_id"] != "temp":
        return None

    match = re.fullmatch(
        r"Prélèvement de ([0-9]+([.,][0-9]*)?) € sur le compte de ([a-z0-9]{7,10})",
        old_log["text"],
    )
    if match is None:
        return None

    return {
        "action": 0x313,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_user_id(mappings, match.group(3)),
            "amount": float(match.group(1).replace(",", ".")),
        },
    }


def acompte_debit(old_log, mappings):
    if old_log["log_category_id"] != "acompte":
        return None

    match = re.fullmatch(
        r"Prélèvement de ([0-9]+([.,][0-9]*)?) € sur ([a-z0-9]{7,10})",
        old_log["text"],
    )
    if match is None:
        return None

    return {
        "action": 0x313,
        "time": old_log["date_at"],
        "client": map_client(mappings),
        "user": map_user_name(mappings, old_log["user"]),
        "details": {
            "id": map_user_id(mappings, match.group(3)),
            "amount": float(match.group(1).replace(",", ".")),
        },
    }


def temp2_ignored(old_log, mappings):
    if old_log["log_category_id"] != "temp2":
        return None

    should_ignore = False
    for ignored_re in [
        r"Ajout de l'événement : .+",
        r"Suppression de l'évènement : .+",
    ]:
        match = re.fullmatch(ignored_re, old_log["text"])
        should_ignore = should_ignore or match is not None

    if should_ignore:
        return []


def vente_ignored(old_log, mappings):
    if old_log["log_category_id"] == "vente":
        return []


def write_line_tsv(output, new_log):
    output.write(f"{new_log['action']}\t")
    output.write(f"{new_log['time']}\t")
    output.write(f"{new_log['client']}\t")
    output.write(f"{new_log['user']}\t")
    output.write(f"{json.dumps(new_log['details'])}\n")


if __name__ == "__main__":
    main()
