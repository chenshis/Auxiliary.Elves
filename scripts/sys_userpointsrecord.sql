
USE auxiliary;
DROP TABLE IF EXISTS sys_userpointsrecord;
CREATE TABLE sys_userpointsrecord(
    `Id` BIGINT NOT NULL COMMENT '主键',
    `UserId` VARCHAR(50) NOT NULL COMMENT '账号唯一编码',
    `UserData` DATETIME COMMENT '积分日期',
    `UserPoints` INT COMMENT '积分',
    `IsExtract` BOOLEAN COMMENT '是否提取',
    `CreateUser` VARCHAR(50) COMMENT '创建人',
    `CreateTime` DATETIME COMMENT '创建时间',
    `UpdateUser` VARCHAR(50) COMMENT '修改人',
    `UpdateTime` DATETIME COMMENT '修改时间',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_userpointsrecord';

