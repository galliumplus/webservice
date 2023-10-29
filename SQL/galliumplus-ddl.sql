-- Création des tables --

USE galliumplus;

ALTER DATABASE `galliumplus`
DEFAULT CHARACTER SET utf8mb4
DEFAULT COLLATE utf8mb4_unicode_520_ci;

CREATE TABLE `User` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `userId` VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    `firstName` VARCHAR(50) NOT NULL,
    `lastName` VARCHAR(50) NOT NULL,
    `email` VARCHAR(80) NOT NULL,
    `role` INTEGER NOT NULL,
    `year` VARCHAR(10) NOT NULL,
    `deposit` DECIMAL(6,2),
    `isMember` BOOLEAN NOT NULL,
    `password` BINARY(32) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
    `salt` CHAR(32) NOT NULL DEFAULT '',
    `registration` DATETIME NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE (`userId`)
);

CREATE TABLE `PasswordResetToken` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `token` VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    `secret` BINARY(32) NOT NULL,
    `salt` CHAR(32) NOT NULL,
    `expiration` DATETIME NOT NULL,
    `userId` VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE (`token`)
);

CREATE TABLE `Role` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `name` VARCHAR(50) NOT NULL,
    `permissions` INTEGER NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `Session` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `token` CHAR(20) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    `lastUse` DATETIME NOT NULL,
    `expiration` DATETIME NOT NULL,
    `user` INTEGER,
    `client` INTEGER NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE (`token`)
);

CREATE TABLE `Client` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `apiKey` CHAR(20) NOT NULL CHARACTER SET ascii COLLATE ascii_bin,
    `name` VARCHAR(50) NOT NULL,
    `granted` INTEGER NOT NULL,
    `revoked` INTEGER NOT NULL,
    `isEnabled` BOOLEAN NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE (`apiKey`)
);

CREATE TABLE `BotClient` (
    `id` INTEGER NOT NULL,
    `secret` BINARY(32) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
    `salt` CHAR(32) NOT NULL DEFAULT '',
    PRIMARY KEY (`id`)
);

CREATE TABLE `SsoClient` (
    `id` INTEGER NOT NULL,
    `secret` CHAR(30) NOT NULL,
    `usesApi` BOOLEAN NOT NULL,
    `redirectUrl` VARCHAR(120) NOT NULL,
    `logoUrl` VARCHAR(120),
    PRIMARY KEY (`id`)
);

CREATE TABLE `Product` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `name` VARCHAR(50) NOT NULL,
    `stock` INTEGER NOT NULL,
    `nonMemberPrice` DECIMAL(6,2) NOT NULL,
    `memberPrice` DECIMAL(6,2) NOT NULL,
    `availability` INTEGER NOT NULL,
    `category` INTEGER NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `Availability` (
    `enum` INTEGER NOT NULL,
    `description` VARCHAR(50),
    PRIMARY KEY (`enum`)
);

CREATE TABLE `Category` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `name` VARCHAR(50) NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `HistoryAction` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `text` VARCHAR(120) NOT NULL,
    `time` DATETIME NOT NULL,
    `kind` INTEGER NOT NULL,
    `actor` INTEGER,
    `target` INTEGER,
    `numericValue` DECIMAL(6,2),
    PRIMARY KEY (`id`)
);

CREATE TABLE `HistoryActionKind` (
    `enum` INTEGER NOT NULL,
    `description` VARCHAR(50),
    PRIMARY KEY (`enum`)
);

CREATE TABLE `HistoryUser` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `userId` VARCHAR(21) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE (`userId`)
);

ALTER TABLE `User`          ADD FOREIGN KEY (`role`)         REFERENCES `Role`(`id`)                ON UPDATE CASCADE  ON DELETE RESTRICT;
ALTER TABLE `PasswordResetToken` ADD FOREIGN KEY (`user`)    REFERENCES `User`(`userId`)            ON UPDATE CASCADE  ON DELETE CASCADE;
ALTER TABLE `Session`       ADD FOREIGN KEY (`user`)         REFERENCES `User`(`id`)                ON UPDATE CASCADE  ON DELETE CASCADE;
ALTER TABLE `Session`       ADD FOREIGN KEY (`client`)       REFERENCES `Client`(`id`)              ON UPDATE CASCADE  ON DELETE CASCADE;
ALTER TABLE `BotClient`     ADD FOREIGN KEY (`id`)           REFERENCES `Client`(`id`)              ON UPDATE CASCADE  ON DELETE CASCADE;
ALTER TABLE `SsoClient`     ADD FOREIGN KEY (`id`)           REFERENCES `Client`(`id`)              ON UPDATE CASCADE  ON DELETE CASCADE;
ALTER TABLE `Product`       ADD FOREIGN KEY (`availability`) REFERENCES `Availability`(`enum`)      ON UPDATE RESTRICT ON DELETE RESTRICT;
ALTER TABLE `Product`       ADD FOREIGN KEY (`category`)     REFERENCES `Category`(`id`)            ON UPDATE CASCADE  ON DELETE RESTRICT;
ALTER TABLE `HistoryAction` ADD FOREIGN KEY (`actor`)        REFERENCES `HistoryUser`(`id`)         ON UPDATE RESTRICT ON DELETE RESTRICT;
ALTER TABLE `HistoryAction` ADD FOREIGN KEY (`target`)       REFERENCES `HistoryUser`(`id`)         ON UPDATE RESTRICT ON DELETE RESTRICT;  
ALTER TABLE `HistoryAction` ADD FOREIGN KEY (`kind`)         REFERENCES `HistoryActionKind`(`enum`) ON UPDATE RESTRICT ON DELETE RESTRICT;
ALTER TABLE `HistoryUser`   ADD FOREIGN KEY (`activeUser`)   REFERENCES `User`(`id`)                ON UPDATE RESTRICT ON DELETE RESTRICT;

SET FOREIGN_KEY_CHECKS = 1;