
USE auxiliary;
DROP TABLE IF EXISTS sys_userpointsrecord;
CREATE TABLE sys_userpointsrecord(
    `Id` BIGINT NOT NULL COMMENT '����',
    `UserId` VARCHAR(50) NOT NULL COMMENT '�˺�Ψһ����',
    `UserData` DATETIME COMMENT '��������',
    `UserPoints` INT COMMENT '����',
    `IsExtract` BOOLEAN COMMENT '�Ƿ���ȡ',
    `CreateUser` VARCHAR(50) COMMENT '������',
    `CreateTime` DATETIME COMMENT '����ʱ��',
    `UpdateUser` VARCHAR(50) COMMENT '�޸���',
    `UpdateTime` DATETIME COMMENT '�޸�ʱ��',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_userpointsrecord';

