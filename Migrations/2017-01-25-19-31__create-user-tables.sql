-- Table: "user"

-- DROP TABLE "user";

CREATE TABLE "user"
(
  id uuid NOT NULL,
  "userName" character(80),
  "name" character(150),
  email character(100),
  password character(150),
  "accessType" character(100),
  CONSTRAINT "PK" PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
