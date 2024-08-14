import React, { useEffect, useState } from 'react';
import axios from 'axios';
import UserAcc from '../Account/User';
import Administrator from './Admin';
import Driver from './Driver';

const RidesList = () => {
  const [rides, setRides] = useState([]);
  const [user, setUser] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRides = async () => {
      const storedUser = JSON.parse(localStorage.getItem('user'));
      const token = storedUser.token;
      console.log(token);
      if (!storedUser || !storedUser.username) {
        setError('No user found in local storage.');
        return;
      }

      try {
        const response = await axios.get('http://localhost:8613/ride/GetUserRides', {
          params: {
            username: storedUser.username
          },
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
        setUser(storedUser);
        
        console.log("Stored user:", token);
        console.log('Response data:', response.data);

        // Ensure the response is an array
        const ridesData = Array.isArray(response.data) ? response.data : [];

        // Store the rides data in localStorage
        localStorage.setItem('rides', JSON.stringify(ridesData));

        // Set the rides state
        setRides(ridesData);
      } catch (err) {
        console.error('Error fetching rides:', err);
        setError('Error fetching rides');
      }
    };

    fetchRides();
  }, []);

  if (error) {
    return <div>{error}</div>;
  }

  // Wait until user data is loaded before rendering user-specific content
  if (user && user.type === "User") {
    return (
      <>
        <div>
          <h1>Ride history</h1>
          {rides.length === 0 ? (
            <p>No rides found.</p>
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
        <UserAcc />
      </>
    );
  }

  if (user && user.type === "Administrator") {
    return (
      <Administrator />
    );
  }
  if (user && user.type === "Driver") {
    return (
      <Driver />
    );
  }

  return <div>Loading...</div>; // Or some other placeholder while the user data is being fetched
};

export default RidesList;
