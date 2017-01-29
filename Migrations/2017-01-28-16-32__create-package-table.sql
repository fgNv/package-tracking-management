CREATE TABLE "package"
(
  id uuid NOT NULL,
  "creatorId" uuid,
  description character(400),
  name character(100) NOT NULL,
  CONSTRAINT primary_key PRIMARY KEY (id),
  "createdAt" timestamp without time zone NOT NULL,
  "updatedAt" timestamp without time zone NOT NULL,
  CONSTRAINT "FK_creator" FOREIGN KEY ("creatorId")
      REFERENCES "user" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);