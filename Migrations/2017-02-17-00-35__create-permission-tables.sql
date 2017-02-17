CREATE IF NOT EXISTS TABLE permission
(
  "userId" uuid NOT NULL,
  "packageId" uuid NOT NULL,
  CONSTRAINT "permission_PK" PRIMARY KEY ("userId", "packageId"),
  CONSTRAINT "permission_package_FK" FOREIGN KEY ("packageId")
      REFERENCES "package" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT "permission_user_FK" FOREIGN KEY ("userId")
      REFERENCES "user" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE permission
  OWNER TO homestead;