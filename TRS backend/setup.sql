CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Email` varchar(255) NOT NULL,
    `Username` varchar(255) NOT NULL,
    `Role` int NOT NULL,
    `PasswordHash` varbinary(1024) NOT NULL,
    `Salt` varbinary(1024) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`)
);


