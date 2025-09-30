
USE auxiliary;
DROP TABLE IF EXISTS sys_serveruser;
CREATE TABLE sys_serveruser(
    `Id` BIGINT NOT NULL COMMENT '主键',
    `UserName` varchar(50) NOT NULL COMMENT '用户名',
    `Password` varchar(255) NOT NULL COMMENT '密码',
    `Status` tinyint(1) NOT NULL COMMENT '是否删除',
    `Role` int NOT NULL COMMENT '预留 暂时都是1 无任何作用',
    `Jti` varchar(50) NOT NULL COMMENT 'jwt id标识',
    `Expires` datetime DEFAULT NULL COMMENT '过期时间',
    `CreateUser` VARCHAR(50) COMMENT '创建人',
    `CreateTime` DATETIME COMMENT '创建时间',
    `UpdateUser` VARCHAR(50) COMMENT '修改人',
    `UpdateTime` DATETIME COMMENT '修改时间',
    PRIMARY KEY (`Id`)
) COMMENT 'sys_serveruser';

