import React, { useEffect, useState } from 'react';
import { fetchRides, updateRideStatus, fetchDriverStatus } from '../api.js';
import User from "../Models/User";
import "../Style/driver.css"
function Driver() {
  const [rides, setRides] = useState([]);
  const [error, setError] = useState(null);
  const [isBanned, setIsBanned] = useState(false);

  useEffect(() => {
    const getRidesAndStatus = async () => {
      try {
        const storedUser = JSON.parse(localStorage.getItem('user'));
        const driver = new User(
          storedUser.username,
          storedUser.email,
          storedUser.password,
          storedUser.name,
          storedUser.lastname,
          storedUser.birthad,
          storedUser.address,
          storedUser.type,
          storedUser.image,
          storedUser.token
       
        );
        // Fetch the rides
        const ridesData = await fetchRides();
        setRides(ridesData);

        // Fetch the driver's status
        const driverStatus = await fetchDriverStatus(storedUser.username); // Assuming this API exists
        setIsBanned(driverStatus.isBanned);
      } catch (err) {
        setError(err.message);
      }
    };

    getRidesAndStatus();
  }, []);

  const handleUpdateRideStatus = async (id, newStatus) => {
    try {
      const success = await updateRideStatus(id, newStatus);
      if (success) {
        setRides(rides.map(ride => 
          ride.id === id ? { ...ride, status: newStatus } : ride
        ));

        if (newStatus === 'in progress') {
          const ride = rides.find(ride => ride.id === id);
          setTimeout(async () => {
            await handleUpdateRideStatus(id, 'done');
          }, ride.waitTime * 60000);
        }
      }
    } catch (err) {
      console.error('There was an error updating the ride status.', err);
    }
  };

  return (
    <div>
      <h2>Available Rides</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {rides.length > 0 ? (
        <ul>
          {rides.map((ride, index) => (
            <li key={index}>
              <p>Start Address: {ride.startAddress}</p>
              <p>Destination: {ride.destination}</p>
              <p>Price: ${ride.price}</p>
              <p>Estimated Wait Time: {ride.waitTime} minutes</p>
              <p>Status: {ride.status}</p>
              {ride.status === 'Available' && (
                <button 
                  onClick={() => handleUpdateRideStatus(ride.id, 'in progress')}
                  disabled={isBanned}  // Disable button if the driver is banned
                >
                  {isBanned ? 'Banned - Cannot Accept Ride' : 'Accept Ride'}
                </button>
              )}
              {ride.status === 'in progress' && <p>Ride is in progress...</p>}
              {ride.status === 'done' && <p>Ride is done</p>}
            </li>
          ))}
        </ul>
      ) : (
        <p>No available rides.</p>
      )}
    </div>
  );
}

export default Driver;
