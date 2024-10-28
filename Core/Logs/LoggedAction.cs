namespace GalliumPlus.Core.Logs;

// premier chiffre = catégorie (1 = paramétrage)
// deuxième chiffre = resource concernée par l'action
// troisième chiffre = type d'action (1 pour créé, 2 pour modifié, 3 pour supprimé, 4 et + pour une action spécifique)

public enum LoggedAction : uint
{
    // CategoryAdded = 0x111,
    // CategoryModified = 0x112,
    // CategoryDeleted = 0x113,

    ClientAdded = 0x121,
    ClientModified = 0x122,
    ClientDeleted = 0x123,
    ClientNewSecretGenerated = 0x124,
    
    // EventAdded = 0x131,
    // EventModified = 0x132,
    // EventDeleted = 0x133,
    
    // ItemAdded = 0x141,
    // ItemModified = 0x142,
    // ItemDeleted = 0x143,
    
    // PaymentMethodAdded = 0x151,
    // PaymentMethodModified = 0x152,
    // PaymentMethodDeleted = 0x153,
    
    // PriceAdded = 0x161,
    // PriceModified = 0x162,
    // PriceDeleted = 0x163,
    
    // PricingTypeAdded = 0x171,
    // PricingTypeModified = 0x172,
    // PricingTypeDeleted = 0x173,
    
    // RoleAdded = 0x181,
    // RoleModified = 0x182,
    // RoleDeleted = 0x183,
    
    // ThirdPartyAdded = 0x191,
    // ThirdPartyModified = 0x192,
    // ThirdPartyDeleted = 0x193,
    
    // UnitAdded = 0x1A1,
    // UnitModified = 0x1A2,
    // UnitDeleted = 0x1A3,
    
    // UserAdded = 0x1B1,
    // UserModified = 0x1B2,
    // UserDeleted = 0x1B3,
}