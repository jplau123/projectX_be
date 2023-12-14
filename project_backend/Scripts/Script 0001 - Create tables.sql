CREATE TABLE users (
user_id serial primary key,
user_name varchar(255) ,
balance int,
role varchar(255),
password varchar(255),
active bool,
created_at date,
created_by varchar(255),
modified_at date,
modified_by varchar(255)

);

CREATE TABLE items (
item_id serial primary key,
item_name varchar(255),
price decimal,
amount int,
created_at date,
created_by varchar(255),
modified_at date,
modified_by varchar(255),
is_deleted boolean

);

CREATE TABLE purchase_history (
purchase_id serial primary key,
user_id int references users(user_id),
item_id int references items(item_id),
price decimal,
created_at date

);


