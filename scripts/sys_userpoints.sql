USE auxiliary;
DROP TABLE IF EXISTS sys_userpoints;
CREATE TABLE sys_userpoints(
    `Id` BIGINT NOT NULL COMMENT '主键',
    `UserId` VARCHAR(50) NOT NULL COMMENT '账号唯一编码',
    `UserPoints` INT COMMENT '积分',
    `CreateUser` VARCHAR(50) COMMENT '创建人',
    `CreateTime` DATETIME COMMENT '创建时间',
    `UpdateUser` VARCHAR(50) COMMENT '修改人',
    `UpdateTime` DATETIME COMMENT '修改时间',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_userpoints';

