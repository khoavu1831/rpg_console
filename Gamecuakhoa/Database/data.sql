INSERT INTO Monsters (name, exp_drop, hp, attack) VALUES
('Slime', 5, 30, 3),
('Goblin', 10, 50, 6),
('Wolf', 15, 60, 8),
('Skeleton', 20, 70, 10),
('Zombie', 25, 80, 12),
('Orc', 30, 120, 15),
('Bat', 8, 25, 4),
('Spider', 12, 40, 7),
('Bandit', 18, 65, 11),
('Lizardman', 28, 90, 14),
('Harpy', 35, 100, 16),
('Minotaur', 50, 200, 25),
('Dark Mage', 45, 150, 20),
('Assassin', 40, 110, 18),
('Giant Rat', 7, 35, 5),
('Fire Elemental', 60, 180, 22),
('Ice Golem', 70, 220, 26),
('Troll', 55, 160, 21),
('Wyvern', 80, 250, 30),
('Demon Lord', 200, 500, 50);

INSERT INTO Items (name, type_item, hp_restore, attack_buff, def_buff, rate_drop) VALUES
('Apple', 'food', 20, 0, 0, 0.20),
('Bread', 'food', 35, 0, 0, 0.15),
('Healing Potion', 'potion', 100, 0, 0, 0.10),
('Elixir', 'potion', 200, 0, 0, 0.05),
('Strength Potion', 'potion', 0, 15, 0, 0.08),
('Defense Potion', 'potion', 0, 0, 15, 0.07),
('Roasted Meat', 'food', 60, 5, 0, 0.12),
('Magic Herb', 'potion', 50, 10, 5, 0.05),
('Energy Drink', 'potion', 30, 10, 0, 0.10),
('Cocacola', 'potion', 0, 0, 25, 0.08);

INSERT INTO Weapons (name, atk, def) VALUES
('Wooden Sword', 5, 0),
('Bronze Sword', 10, 2),
('Iron Sword', 15, 4),
('Steel Sword', 22, 6),
('Knight Shield', 0, 12),
('Bronze Shield', 0, 6),
('Leather Armor', 2, 8),
('Chainmail', 4, 12),
('War Axe', 25, -2);