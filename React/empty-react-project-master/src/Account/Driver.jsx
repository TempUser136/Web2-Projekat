import React, { useEffect, useState } from 'react';
import axios from 'axios';

function Driver() {
  const [rides, setRides] = useState([]);
  const [error, setError] = useState(null);

  // Fetch rides when the component mounts
  useEffect(() => {
    const fetchRides = async () => {
      try {
        const response = await axios.get('http://localhost:8613/ride/GetAllRides');
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

  // Update ride status
  const updateRideStatus = async (id, newStatus) => {
    const rideUpdateDto = {
      id: id,  // Ride ID
      status: newStatus // New status
    };

    try {
      await axios.post('http://localhost:8613/ride/UpdateRideStatus', rideUpdateDto);
      
      // Update the status locally in the state
      setRides(rides.map(ride => 
        ride.id === id ? { ...ride, status: newStatus } : ride
      ));

      if (newStatus === 'in progress') {
        const ride = rides.find(ride => ride.id === id);
        
        // Set a timer to update the ride status to "done" after the wait time
        setTimeout(async () => {
          await updateRideStatus(id, 'done');
        }, ride.waitTime * 60000); // Convert minutes to milliseconds
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
                <button onClick={() => updateRideStatus(ride.id, 'in progress')}>
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
