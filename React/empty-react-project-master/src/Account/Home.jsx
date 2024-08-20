import React, { useEffect, useState } from 'react';
import { getUserRides } from './rideservice.js';
import UserAcc from '../Account/User';
import Administrator from './Admin';
import Driver from './Driver';
import { useNavigate } from 'react-router-dom';

const RidesList = () => {
  const [rides, setRides] = useState([]);
  const [user, setUser] = useState(null);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchRides = async () => {
      const storedUser = JSON.parse(localStorage.getItem('user'));
      const token = storedUser.token;
      if (!storedUser || !storedUser.username) {
        setError('No user found in local storage.');
        return;
      }

      try {
        const ridesData = await getUserRides(storedUser.username, token);
        setUser(storedUser);
        localStorage.setItem('rides', JSON.stringify(ridesData));
        setRides(ridesData);
      } catch (err) {
        setError('Error fetching rides');
      }
    };

    fetchRides();
  }, []);

  if (error) {
    return <div>{error}</div>;
  }

  const handleProfileClick = () => {
    navigate('/Profile');
  };

  return (
    <>
      <div>
        <button onClick={handleProfileClick}>Profile</button>
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
      {user && user.type === "User" && <UserAcc />}
      {user && user.type === "Administrator" && <Administrator />}
      {user && user.type === "Driver" && <Driver />}
    </>
  );
};

export default RidesList;
