USE auxiliary;
DROP TABLE IF EXISTS sys_userpoints;
CREATE TABLE sys_userpoints(
    `Id` BIGINT NOT NULL COMMENT '����',
    `UserId` VARCHAR(50) NOT NULL COMMENT '�˺�Ψһ����',
    `UserPoints` INT COMMENT '����',
    `CreateUser` VARCHAR(50) COMMENT '������',
    `CreateTime` DATETIME COMMENT '����ʱ��',
    `UpdateUser` VARCHAR(50) COMMENT '�޸���',
    `UpdateTime` DATETIME COMMENT '�޸�ʱ��',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_userpoints';

