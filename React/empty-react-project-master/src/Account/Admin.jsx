import React, { useEffect, useState } from 'react';
import axios from 'axios';

const Administrator = () => {
  const [drivers, setDrivers] = useState([]);
  const [error, setError] = useState(null);
  const [rides, setRides] = useState([]);


  useEffect(() => {
    const fetchDrivers = async () => {
      try {
        const response = await axios.get('http://localhost:8613/user/GetDrivers');
        console.log(response.data); // Log the response to check the data

        setDrivers(response.data);
      } catch (err) {
        setError('Error fetching drivers');
      }
    };

    const fetchRides = async () => {
      try {
        const response = await axios.get('http://localhost:8613/ride/GetAllRides');


        // Ensure the response is an array
        const ridesData = Array.isArray(response.data) ? response.data : [];

        // Store the rides data in localStorage

        // Set the rides state
        setRides(ridesData);
      } catch (err) {
        console.error('Error fetching rides:', err);
        setError('Error fetching rides');
      }
    };

    fetchRides();

    fetchDrivers();
  }, []);

  if (error) {
    return <div>{error}</div>;
  }

  return (
    <>
     <div>
          <h1>Ride history</h1>
          {rides.length === 0 ? (
            <p>Admin no rides found.</p>
          ) : (
            <ul>
              {rides.map((ride, index) => (
                <li key={index}>
                  Start Address: {ride.startAddress}, Destination: {ride.destination}, Price: {ride.price}, Wait Time: {ride.waitTime}, Status: {ride.status}
                </li>
              ))}
            </ul>
          )}
        </div>
    <div>
      <h1>Driver List</h1>
      <ul>
        {drivers.map((driver, index) => (
          <li key={index}>
            Username: {driver.Username}, Email: {driver.email}
          </li>
        ))}
      </ul>
    </div>
    </>
    
  );
};

export default Administrator;
