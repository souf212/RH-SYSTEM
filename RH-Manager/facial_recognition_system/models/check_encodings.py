import sys
import os
import pickle

# Add the `src/` directory to the Python path
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), "../src")))

from config import ENCODINGS_FILE  # Now Python can find config.py

# Load encodings from the file
with open(ENCODINGS_FILE, "rb") as f:
    data = pickle.load(f)

# Extract stored names
known_names = data.get("names", [])

# Print the stored names
print("🔍 Stored Names in Encodings File:")
for name in known_names:
    print(name)
