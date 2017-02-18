CREATE TABLE IF NOT EXISTS permission
(
  user_id uuid NOT NULL,
  package_id uuid NOT NULL,
  CONSTRAINT "permission_PK" PRIMARY KEY (user_id, package_id),
  CONSTRAINT "permission_package_FK" FOREIGN KEY (package_id)
      REFERENCES "package" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT "permission_user_FK" FOREIGN KEY (user_id)
      REFERENCES "user" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);