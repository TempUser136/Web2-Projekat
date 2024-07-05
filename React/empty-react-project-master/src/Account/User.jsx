import React, { useState } from 'react';
import axios from 'axios';

function AddressForm() {
  const [startAddress, setStartAddress] = useState('');
  const [destination, setDestination] = useState('');
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('https://localhost:7280/api/users/calculate', {
        startAddress,
        destination
      }, {
        withCredentials: true // Include credentials in the request
      });

      console.log('Response data:', response.data); // Log the response data

      setResult(response.data);
      setError(null);
    } catch (err) {
      console.error('There was an error calculating the price and wait time.', err); // Log the error
      setError('There was an error calculating the price and wait time.');
      setResult(null);
    }
  };

  const handleAccept = async () => {
    try {
      await axios.post('https://localhost:7280/api/rides/addRide', {
        startAddress,
        destination,
        price: result.price,
        waitTime: result.waitTime,
        status: "Available"
      }, {
        withCredentials: true // Include credentials in the request
      });

      alert('You have accepted the ride!');
      // Clear the form and result
      setStartAddress('');
      setDestination('');
      setResult(null);
    } catch (error) {
      console.error('There was an error accepting the ride!', error);
    }
  };

  const handleDecline = () => {
    alert('You have declined the ride.');
    // Clear the form and result
    setStartAddress('');
    setDestination('');
    setResult(null);
  };

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Start Address:</label>
          <input
            type="text"
            value={startAddress}
            onChange={(e) => setStartAddress(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Destination:</label>
          <input
            type="text"
            value={destination}
            onChange={(e) => setDestination(e.target.value)}
            required
          />
        </div>
        <button type="submit">Calculate</button>
      </form>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      {result && (
        <div>
          <p>Price: ${result.price}</p>
          <p>Estimated Wait Time: {result.waitTime} minutes</p>
          <button onClick={handleAccept}>Accept</button>
          <button onClick={handleDecline}>Decline</button>
        </div>
      )}
    </div>
  );
}

export default AddressForm;
