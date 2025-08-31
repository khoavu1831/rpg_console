create database if not exists rpg_console;

create table if not exists Players(
	id int auto_increment primary key,
	name varchar(30),
	level int default 1,
	exp int default 0,
	atk int default 10,
	def int default 5,
	hp int default 100,
	monster_killed int default 0
);

create table if not exists Monsters(
	id int auto_increment primary key,
	name varchar(30),
	exp_drop int,
	hp int,
	attack int
);

create table if not exists Items(
	id int auto_increment primary key,
	name varchar(30),
	type_item varchar(10),
	hp_restore int,
	attack_buff int,
	def_buff int,
	rate_drop double
);

create table if not exists Weapons(
	id int auto_increment primary key,
	name varchar(20),
	atk int,
	def int
);

create table if not exists InventoryItems(
	id int auto_increment primary key,
	player_id int not null,
	item_id int not null,
	quantity int not null default 1,
	foreign key (player_id) references Players(id),
	foreign key (item_id) references Items(id),
	unique key uq_player_item (player_id, item_id)
);

create table if not exists InventoryWeapons(
	id int auto_increment primary key,
	player_id int not null,
	weapon_id int not null,
	is_equipped tinyint(1) not null default 0,
	foreign key (player_id) references Players(id),
	foreign key (weapon_id) references Weapons(id)
);

-- create table Quest();