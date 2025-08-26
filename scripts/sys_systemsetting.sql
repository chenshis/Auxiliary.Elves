USE auxiliary;
DROP TABLE IF EXISTS sys_systemsetting;
CREATE TABLE sys_systemsetting(
    `Id` BIGINT NOT NULL COMMENT '����',
    `HandlingFee` INT COMMENT '������',
    `MinNumber` INT COMMENT '�������',
    `Mininterval` INT COMMENT '��С����',
    `Maxinterval` INT COMMENT '�������',
    `CreateUser` VARCHAR(50) COMMENT '������',
    `CreateTime` DATETIME COMMENT '����ʱ��',
    `UpdateUser` VARCHAR(50) COMMENT '�޸���',
    `UpdateTime` DATETIME COMMENT '�޸�ʱ��',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_systemsetting';

