USE auxiliary;
DROP TABLE IF EXISTS sys_systemsetting;
CREATE TABLE sys_systemsetting(
    `Id` BIGINT NOT NULL COMMENT '主键',
    `HandlingFee` INT COMMENT '手续费',
    `MinNumber` INT COMMENT '最低数量',
    `Mininterval` INT COMMENT '最小区间',
    `Maxinterval` INT COMMENT '最大区间',
    `CreateUser` VARCHAR(50) COMMENT '创建人',
    `CreateTime` DATETIME COMMENT '创建时间',
    `UpdateUser` VARCHAR(50) COMMENT '修改人',
    `UpdateTime` DATETIME COMMENT '修改时间',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_systemsetting';

