\restrict 9mkLCjWhhePvfzJ3t9GZ3qhnnrmC3tuJGFNaoWhHCCOwodP1DmZ0Qp5SPunPd09

-- Dumped from database version 18.1
-- Dumped by pg_dump version 18.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: migration; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA migration;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: schema_migrations; Type: TABLE; Schema: migration; Owner: -
--

CREATE TABLE migration.schema_migrations (
    version character varying NOT NULL
);


--
-- Name: memberships; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.memberships (
    player_id uuid NOT NULL,
    squad_id uuid NOT NULL
);


--
-- Name: players; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.players (
    id uuid NOT NULL,
    name text NOT NULL,
    "character" text,
    room_id uuid NOT NULL
);


--
-- Name: quest_votes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.quest_votes (
    value boolean NOT NULL,
    player_id uuid NOT NULL,
    squad_id uuid NOT NULL
);


--
-- Name: rooms; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.rooms (
    id uuid NOT NULL,
    status text NOT NULL
);


--
-- Name: squad_votes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.squad_votes (
    value boolean NOT NULL,
    player_id uuid NOT NULL,
    squad_id uuid NOT NULL
);


--
-- Name: squads; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.squads (
    id uuid NOT NULL,
    status text NOT NULL,
    quest_number integer NOT NULL,
    squad_number integer NOT NULL,
    required_members_number integer NOT NULL,
    is_double_fail boolean NOT NULL,
    room_id uuid NOT NULL
);


--
-- Name: schema_migrations schema_migrations_pkey; Type: CONSTRAINT; Schema: migration; Owner: -
--

ALTER TABLE ONLY migration.schema_migrations
    ADD CONSTRAINT schema_migrations_pkey PRIMARY KEY (version);


--
-- Name: memberships memberships_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.memberships
    ADD CONSTRAINT memberships_pkey PRIMARY KEY (player_id, squad_id);


--
-- Name: players players_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.players
    ADD CONSTRAINT players_pkey PRIMARY KEY (id);


--
-- Name: players players_room_id_name_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.players
    ADD CONSTRAINT players_room_id_name_key UNIQUE (room_id, name);


--
-- Name: quest_votes quest_votes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.quest_votes
    ADD CONSTRAINT quest_votes_pkey PRIMARY KEY (player_id, squad_id);


--
-- Name: rooms rooms_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.rooms
    ADD CONSTRAINT rooms_pkey PRIMARY KEY (id);


--
-- Name: squad_votes squad_votes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.squad_votes
    ADD CONSTRAINT squad_votes_pkey PRIMARY KEY (player_id, squad_id);


--
-- Name: squads squads_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.squads
    ADD CONSTRAINT squads_pkey PRIMARY KEY (id);


--
-- Name: squads squads_room_id_quest_number_squad_number_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.squads
    ADD CONSTRAINT squads_room_id_quest_number_squad_number_key UNIQUE (room_id, quest_number, squad_number);


--
-- Name: idx_players_room; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX idx_players_room ON public.players USING btree (room_id);


--
-- Name: idx_squads_room; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX idx_squads_room ON public.squads USING btree (room_id);


--
-- Name: memberships memberships_player_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.memberships
    ADD CONSTRAINT memberships_player_id_fkey FOREIGN KEY (player_id) REFERENCES public.players(id);


--
-- Name: memberships memberships_squad_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.memberships
    ADD CONSTRAINT memberships_squad_id_fkey FOREIGN KEY (squad_id) REFERENCES public.squads(id);


--
-- Name: players players_room_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.players
    ADD CONSTRAINT players_room_id_fkey FOREIGN KEY (room_id) REFERENCES public.rooms(id);


--
-- Name: quest_votes quest_votes_player_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.quest_votes
    ADD CONSTRAINT quest_votes_player_id_fkey FOREIGN KEY (player_id) REFERENCES public.players(id);


--
-- Name: quest_votes quest_votes_squad_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.quest_votes
    ADD CONSTRAINT quest_votes_squad_id_fkey FOREIGN KEY (squad_id) REFERENCES public.squads(id);


--
-- Name: squad_votes squad_votes_player_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.squad_votes
    ADD CONSTRAINT squad_votes_player_id_fkey FOREIGN KEY (player_id) REFERENCES public.players(id);


--
-- Name: squad_votes squad_votes_squad_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.squad_votes
    ADD CONSTRAINT squad_votes_squad_id_fkey FOREIGN KEY (squad_id) REFERENCES public.squads(id);


--
-- Name: squads squads_room_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.squads
    ADD CONSTRAINT squads_room_id_fkey FOREIGN KEY (room_id) REFERENCES public.rooms(id);


--
-- PostgreSQL database dump complete
--

\unrestrict 9mkLCjWhhePvfzJ3t9GZ3qhnnrmC3tuJGFNaoWhHCCOwodP1DmZ0Qp5SPunPd09


--
-- Dbmate schema migrations
--

INSERT INTO migration.schema_migrations (version) VALUES
    ('20251214205809');
