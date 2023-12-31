﻿CREATE TABLE users (
user_id serial primary key,
user_name varchar(255) UNIQUE,
balance decimal default 0,
role varchar(255),
password varchar(255),
active bool default true,
is_deleted bool default false,
created_at timestamp default current_timestamp,
created_by varchar(255),
modified_at timestamp,
modified_by varchar(255),
is_deleted boolean default false,
token varchar(255),
token_created_at timestamp,
token_expires timestamp
);

CREATE TABLE items (
item_id serial primary key,
item_name varchar(255) UNIQUE,
price decimal,
quantity int,
created_at timestamp default current_timestamp,
created_by varchar(255),
modified_at timestamp,
modified_by varchar(255),
is_deleted boolean default false
);

CREATE TABLE purchase_history (
purchase_id serial primary key,
user_id int references users(user_id),
item_id int references items(item_id),
quantity int,
unit_price decimal,
created_at timestamp default current_timestamp
);


