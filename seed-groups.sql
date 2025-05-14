-- Delete existing groups
DELETE FROM Groups;

-- Reset the auto-increment counter
DELETE FROM sqlite_sequence WHERE name='Groups';

-- Insert new groups
INSERT INTO Groups (Code, Description, Address1, City, State, Zip, Country)
VALUES 
('HQ', 'Headquarters', '123 Main Street', 'New York', 'NY', '10001', 'USA'),
('WEST', 'West Coast Office', '456 Tech Blvd', 'San Francisco', 'CA', '94107', 'USA'),
('SOUTH', 'South Region Office', '789 Palm Drive', 'Miami', 'FL', '33101', 'USA');
