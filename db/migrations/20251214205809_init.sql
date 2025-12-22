-- migrate:up
create table rooms (
    id      uuid    primary key,
    status  text    not null
);

create table players (
    id          uuid    primary key,
    name        text    not null,
    character   text    not null,

    room_id     uuid    not null references rooms(id),

    unique (room_id, name)
);

create table squads (
    id                      uuid    primary key,
    status                  text    not null,
    quest_number            int     not null,
    squad_number            int     not null,
    required_members_number int     not null,
    is_double_fail          boolean not null,

    room_id                 uuid    not null references rooms(id),

    unique (room_id, quest_number, squad_number)
);

create table memberships (
    player_id   uuid    not null references players(id),
    squad_id    uuid    not null references squads(id),
    primary key (player_id, squad_id)
);

create table squad_votes (
    value       boolean not null,

    player_id   uuid    not null references players(id),
    squad_id    uuid    not null references squads(id),
    primary key (player_id, squad_id)
);

create table quest_votes (
    value       boolean not null,

    player_id   uuid    not null references players(id),
    squad_id    uuid    not null references squads(id),
    primary key (player_id, squad_id)
);

create index idx_players_room on players(room_id);
create index idx_squads_room on squads(room_id);

-- migrate:down
drop index if exists idx_players_room;
drop index if exists idx_squads_room;

drop table if exists quest_votes;
drop table if exists squad_votes;
drop table if exists memberships;
drop table if exists squads;
drop table if exists players;
drop table if exists rooms;
