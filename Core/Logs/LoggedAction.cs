namespace GalliumPlus.Core.Logs;

// 1xx = paramétrage
//    deuxième chiffre = resource concernée par l'action
//    1x1 = créé, 1x2 = modifié, 1x3 = supprimé, 1x4-1xF pour toutes les autres opérations
// 2xx = opérations sur le stock
// 3xx = paiements

public enum LoggedAction : uint
{
    CategoryAdded = 0x111,
    CategoryModified = 0x112,
    CategoryDeleted = 0x113,

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
    
    UserAdded = 0x1B1,
    UserModified = 0x1B2,
    UserDeleted = 0x1B3,
    // UserDepositOpen = 0x1B4,
    UserDepositClosed = 0x1B5,
    
    // AdvanceDeposited = 0x3XX,
    // AdvanceWithdrawn = 0x3XX,
}