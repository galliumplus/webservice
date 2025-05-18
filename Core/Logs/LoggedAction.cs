namespace GalliumPlus.Core.Logs;

// 1xx = paramétrage
//    deuxième chiffre = resource concernée par l'action
//    1x1 = créé, 1x2 = modifié, 1x3 = supprimé
// 2xx = opérations sur le stock
//    2x1 = entrée, 2x2 = interne, 2x3 = sortie
// 3xx = paiements
// 4xx = connexions
//    41x = connexions entrantes
//    42x = connexions sortantes

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
    
    PriceListAdded = 0x171,
    PriceListModified = 0x172,
    PriceListDeleted = 0x173,
    
    RoleAdded = 0x181,
    RoleModified = 0x182,
    RoleDeleted = 0x183,
    
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

    // ForcedStockIn = 0x2F1
    // ForcedStockOut = 0x2F3
    
    // AdvanceDeposited = 0x311,
    // AdvanceWithdrawn = 0x313,
    
    UserLoggedIn = 0x411,
    ApplicationConnected = 0x412,
    SsoUserLoggedIn = 0x413,
}