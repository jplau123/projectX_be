CREATE OR REPLACE FUNCTION update_modified_at()
RETURNS TRIGGER AS $$
BEGIN
	NEW.modified_at = NOW();
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER user_modified
BEFORE UPDATE ON users
FOR EACH ROW
EXECUTE FUNCTION update_modified_at();

CREATE TRIGGER item_modified
BEFORE UPDATE ON items
FOR EACH ROW
EXECUTE FUNCTION update_modified_at();