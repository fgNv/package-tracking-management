CREATE TABLE device
(
  id uuid NOT NULL,
  name character varying(100) NOT NULL,
  "cretorId" uuid NOT NULL,
  CONSTRAINT "PK_device" PRIMARY KEY (id),
  CONSTRAINT "FK_device_creator" FOREIGN KEY ("cretorId")
      REFERENCES "user" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);

CREATE TABLE manual_point
(
  "creatorId" uuid,
  id uuid NOT NULL,
  "packageId" uuid,
  coordinates point NOT NULL,
  "createdAt" timestamp without time zone NOT NULL,
  CONSTRAINT "PK_manual_point" PRIMARY KEY (id),
  CONSTRAINT "FK_creator" FOREIGN KEY ("creatorId")
      REFERENCES "user" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT "FK_package" FOREIGN KEY ("packageId")
      REFERENCES "package" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);

CREATE TABLE device_point
(
  id uuid NOT NULL,
  coordinates point NOT NULL,
  device_id uuid NOT NULL,
  package_id uuid NOT NULL,
  "createdAt" timestamp without time zone NOT NULL,
  CONSTRAINT "PK_device_point" PRIMARY KEY (id),
  CONSTRAINT "FK_device_point_package" FOREIGN KEY (package_id)
      REFERENCES "package" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT "FK_point_device" FOREIGN KEY (device_id)
      REFERENCES device (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);