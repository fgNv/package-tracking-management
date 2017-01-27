-- Table: "user"

-- DROP TABLE "user";

CREATE TABLE "user"
(
  id uuid NOT NULL,
  "userName" character(80) NOT NULL,
  email character(100) NOT NULL,
  password character(150) NOT NULL,
  "accessType" character(100) NOT NULL,
  name character(150) NOT NULL,
  CONSTRAINT "PK" PRIMARY KEY (id),
  CONSTRAINT "unique-email" UNIQUE (email),
  CONSTRAINT "unique-username" UNIQUE ("userName")
)
WITH (
  OIDS=FALSE
);
