create database if not exists rpg_console;

create table Players(
    id int auto_increment primary key,
    name varchar(30),
    level int default 1,
    exp int default 0,
    atk int default 10,
    hp int default 100,
    monster_killed int default 0
);

create table Monsters(
    id int auto_increment primary key,
    name varchar(30),
    exp_drop int,
    hp int,
    attack int
);

create table Items(
    id int auto_increment primary key,
    name varchar(30),
    type_item varchar(10),
    hp_restore int,
    attack_buff int,
    def_buff int,
    rate_drop double
);

create table Weapons(
    id int auto_increment primary key,
    name varchar(20),
    atk int,
    def int
);

create table Inventories(
    id int auto_increment primary key,
    player_id int,
    item_id int,
    weapon_id int,
    foreign key (player_id) references Players(id),
    foreign key (item_id) references Items(id),
    foreign key (weapon_id) references Weapons(id)
);

-- create table Quest();
