import React, { useEffect, useState } from 'react';
import axios from 'axios';

function Driver() {
  const [rides, setRides] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRides = async () => {
      try {
        const response = await axios.get('https://localhost:7280/api/rides/available', {
          withCredentials: true // Include credentials in the request
        });
        console.log('Fetched Rides:', response.data);

        if (response.data) {
          setRides(response.data);
        } else {
          setRides([]);
        }

        setError(null);
      } catch (err) {
        console.error('There was an error fetching the rides.', err);
        setError('There was an error fetching the rides.');
      }
    };

    fetchRides();
  }, []);

  useEffect(() => {
    console.log('Rides state updated:', rides);
  }, [rides]);

  const acceptRide = async (ride) => {
    try {
      await axios.post('https://localhost:7280/api/rides/updateStatus', {
        startAddress: ride.startAddress,
        destination: ride.destination,
        status: 'in progress'
      }, {
        withCredentials: true
      });
      setRides(rides.map(r => r.startAddress === ride.startAddress && r.destination === ride.destination ? { ...r, status: 'in progress' } : r));

      // Set a timer to update the ride status to "done" after the wait time
      setTimeout(async () => {
        await axios.post('https://localhost:7280/api/rides/updateStatus', {
          startAddress: ride.startAddress,
          destination: ride.destination,
          status: 'done'
        }, {
          withCredentials: true
        });
        setRides(rides.map(r => r.startAddress === ride.startAddress && r.destination === ride.destination ? { ...r, status: 'done' } : r));
      }, ride.waitTime * 60000); // Convert minutes to milliseconds
    } catch (err) {
      console.error('There was an error accepting the ride.', err);
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
              {ride.status === 'available' && <button onClick={() => acceptRide(ride)}>Accept Ride</button>}
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
