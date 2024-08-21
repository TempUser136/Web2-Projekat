import React, { useState, useEffect } from 'react';
import { calculateRide, addRide } from './rideservice.js';
import Ride from "../Models/Ride";
import "../Style/user.css"
function AddressForm() {
  const [startAddress, setStartAddress] = useState('');
  const [destination, setDestination] = useState('');
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [isRideAccepted, setIsRideAccepted] = useState(false);
  const [timer, setTimer] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const rideResult = await calculateRide(startAddress, destination);
      setResult(rideResult);
      setError(null);
    } catch (err) {
      setError('There was an error calculating the price and wait time.');
      setResult(null);
    }
  };

  const handleAccept = async () => {
    const ride = new Ride(
      startAddress,
      destination,
      result.waitTime,
      result.price,
      "available"
    );

    try {
      const storedUser = JSON.parse(localStorage.getItem('user'));

      await addRide({
        startAddress,
        destination,
        price: result.price,
        waitTime: result.waitTime,
        status: "Available",
        username: storedUser.username
      });

      alert('You have accepted the ride!');
      setIsRideAccepted(true);

      const waitTimeInMilliseconds = result.waitTime * 60000;
      setTimer(waitTimeInMilliseconds);

      setStartAddress('');
      setDestination('');
      setResult(null);
    } catch (error) {
      console.error('There was an error accepting the ride!', error);
    }
  };

  const handleDecline = () => {
    alert('You have declined the ride.');
    setStartAddress('');
    setDestination('');
    setResult(null);
  };

  useEffect(() => {
    if (isRideAccepted && timer !== null) {
      const countdown = setInterval(() => {
        setTimer(prevTimer => {
          if (prevTimer <= 1000) {
            clearInterval(countdown);
            setIsRideAccepted(false);
            setTimer(null);
            return null;
          }
          return prevTimer - 1000;
        });
      }, 1000);

      return () => clearInterval(countdown);
    }
  }, [isRideAccepted, timer]);

  const formatTime = (milliseconds) => {
    const minutes = Math.floor(milliseconds / 60000);
    const seconds = Math.floor((milliseconds % 60000) / 1000);
    return `${minutes}m ${seconds}s`;
  };

  return (
    <div>
      <form onSubmit={handleSubmit} disabled={isRideAccepted}>
        <div>
          <label>Start Address:</label>
          <input
            type="text"
            name="Start"
            value={startAddress}
            onChange={(e) => setStartAddress(e.target.value)}
            required
            disabled={isRideAccepted}
          />
        </div>
        <div>
          <label>Destination:</label>
          <input
            type="text"
            name="Destination"
            value={destination}
            onChange={(e) => setDestination(e.target.value)}
            required
            disabled={isRideAccepted}
          />
        </div>
        <button type="submit" disabled={isRideAccepted}>Calculate</button>
      </form>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      {result && (
        <div>
          <p>Price: ${result.price}</p>
          <p>Estimated Wait Time: {result.waitTime} minutes</p>
          <button onClick={handleAccept} disabled={isRideAccepted}>Accept</button>
          <button onClick={handleDecline} disabled={isRideAccepted}>Decline</button>
        </div>
      )}

      {isRideAccepted && timer !== null && (
        <div>
          <p>Ride accepted! Please wait for {formatTime(timer)} before performing another action.</p>
        </div>
      )}
    </div>
  );
}

export default AddressForm;
