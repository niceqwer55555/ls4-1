const express = require('express');
const path = require('path');

// Create an instance of Express
const app = express();

// Define the port to run the server on
const PORT = process.env.PORT || 8081;

// Serve static files from the 'public' directory
app.use('/cdn', express.static(path.join(__dirname, 'public')));

// Start the server
app.listen(PORT, () => {
	console.log(`Server is running on port ${PORT}`);
});