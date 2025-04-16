import subprocess
import sys
import re

result = subprocess.run(
    ["git", "diff", "-GSetVersion", "origin/main", "--", "WebService/Program.cs"],
    capture_output=True,
    text=True,
)
output = result.stdout

if not output:
    print("ERREUR: La version du serveur n'a pas l'air d'avoir changé.")
    sys.exit(1)

lines = output.split("\n")
old_version = None
new_version = None
for line in lines:
    match_old_version = re.fullmatch(
        r"-\s*ServerInfo\.Current\.SetVersion\((\d+),\s*(\d+),\s*(\d+).*\);\s*", line
    )
    if match_old_version:
        old_version = tuple(int(g) for g in match_old_version.groups())
    match_new_version = re.fullmatch(
        r"\+\s*ServerInfo\.Current\.SetVersion\((\d+),\s*(\d+),\s*(\d+).*\);\s*", line
    )
    if match_new_version:
        new_version = tuple(int(g) for g in match_new_version.groups())

if old_version is None or new_version is None:
    print("ERREUR: La version du serveur n'a pas pu être lue.")
    sys.exit(1)

print(f"Version actuelle :  v{old_version[0]}.{old_version[1]}.{old_version[2]}")
print(f"Nouvelle version :  v{new_version[0]}.{new_version[1]}.{new_version[2]}")

if old_version[0] != 1 or new_version[0] != 1:
    print("ERREUR: La version majeure du serveur doit être 1.")
    sys.exit(1)

if new_version[1] < old_version[1]:
    print("ERREUR: La version mineure ne doit pas régresser.")
    sys.exit(1)

elif new_version[1] == old_version[1]:
    if new_version[2] < old_version[2]:
        print("ERREUR: La version corrective ne doit pas régresser.")
        sys.exit(1)

    if new_version[2] == old_version[2]:
        print("ERREUR: La nouvelle version doit être différente.")
        sys.exit(1)

    print("OK, montée de version corrective.")

else:
    if new_version[2] != 0:
        print(
            "ERREUR: La version corrective doit revenir à zéro lors d'une mise à jour mineure."
        )
        sys.exit(1)

    print("OK, montée de version mineure.")
