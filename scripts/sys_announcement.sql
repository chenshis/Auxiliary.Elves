USE auxiliary;

DROP TABLE IF EXISTS sys_announcement;
CREATE TABLE sys_announcement(
    `Id` BIGINT NOT NULL COMMENT '主键',
    `Announcement` TEXT NOT NULL COMMENT '公告内容',
    `CreateUser` VARCHAR(50) COMMENT '创建人',
    `CreateTime` DATETIME COMMENT '创建时间',
    `UpdateUser` VARCHAR(50) COMMENT '修改人',
    `UpdateTime` DATETIME COMMENT '修改时间',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_announcement';

