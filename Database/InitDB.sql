create database if not exists rpg_console;

create table Players(
    id_player int auto_increment primary key,
    name varchar(30),
    level int default(1),
    exp int,
    attack int,
    hp int,
    monster_killed int
);

create table Monsters(
    id_monster int auto_increment primary key,
    name varchar(30),
    exp_drop int,
    hp int,
    attack int
);

create table Inventories(
    id_inventory int auto_increment primary key,
    id_player int,
    foreign key (id_player) references Players(id_player)
);

create table Items(
    id_item int auto_increment primary key,
    name varchar(30),
    type_item varchar(10),
    hp_restore int,
    attack_buff int,
    def_buff int,
    id_inventory int,
    foreign key (id_inventory) references Inventories(id_inventory)
);

create table Weapons(
    name varchar(20) primary key,
    attack_buff int,
    def_buff int,
    id_inventory int,
    foreign key (id_inventory) references Inventories(id_inventory)
);

-- create table Quest();
