-- Table: "user"

-- DROP TABLE "user";

CREATE TABLE "user"
(
  id uuid NOT NULL,
  "userName" character(80),
  email character(100),
  password character(150),
  "accessType" character(100),
  name character(150),
  CONSTRAINT "PK" PRIMARY KEY (id),
  CONSTRAINT "unique-email" UNIQUE (email),
  CONSTRAINT "unique-username" UNIQUE ("userName")
)
WITH (
  OIDS=FALSE
);
