import React, { useEffect, useState } from 'react';
import { fetchRides, updateRideStatus } from '../api.js';

function Driver() {
  const [rides, setRides] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const getRides = async () => {
      try {
        const ridesData = await fetchRides();
        setRides(ridesData);
      } catch (err) {
        setError(err.message);
      }
    };

    getRides();
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
                <button onClick={() => handleUpdateRideStatus(ride.id, 'in progress')}>
                  Accept Ride
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
