CREATE TABLE member (
    id SERIAL PRIMARY KEY,               -- Auto-incrementing primary key
    first_name VARCHAR(50) NOT NULL,     -- Member's first name (max 50 characters)
    last_name VARCHAR(50) NOT NULL,      -- Member's last name (max 50 characters)
    email VARCHAR(100) UNIQUE NOT NULL,  -- Member's email (must be unique)
    phone_number VARCHAR(15),            -- Optional phone number (max 15 characters)
    date_of_birth DATE,                  -- Optional date of birth field
    joined_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,  -- When the member joined (defaults to current time)
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP, -- Automatically updates the timestamp when a record is modified
    status VARCHAR(20) DEFAULT 'active', -- Membership status (e.g., 'active', 'inactive', etc.)
    CONSTRAINT email_format CHECK (email LIKE '%_@__%.__%')  -- Basic email format validation
);

-- Optional: Index on the email field to speed up queries by email
CREATE INDEX idx_member_email ON member(email);

-- Create the trigger function
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP; 
    RETURN NEW; 
END;
$$ LANGUAGE plpgsql;

-- Create the trigger
CREATE TRIGGER update_member_updated_at
BEFORE UPDATE ON member
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();