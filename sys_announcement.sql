USE auxiliary;

DROP TABLE IF EXISTS sys_announcement;
CREATE TABLE sys_announcement(
    `Id` BIGINT NOT NULL COMMENT '����',
    `Announcement` TEXT NOT NULL COMMENT '��������',
    `CreateUser` VARCHAR(50) COMMENT '������',
    `CreateTime` DATETIME COMMENT '����ʱ��',
    `UpdateUser` VARCHAR(50) COMMENT '�޸���',
    `UpdateTime` DATETIME COMMENT '�޸�ʱ��',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_announcement';

