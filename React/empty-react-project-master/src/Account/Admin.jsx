import React, { useEffect, useState } from 'react';
import { fetchDrivers, fetchRides, approveDriver, declineDriver, blockDriver, unblockDriver } from '../api.js';

const Administrator = () => {
  const [drivers, setDrivers] = useState([]);
  const [error, setError] = useState(null);
  const [rides, setRides] = useState([]);

  useEffect(() => {
    const getDriversAndRides = async () => {
      try {
        const driversData = await fetchDrivers();
        setDrivers(driversData);

        const ridesData = await fetchRides();
        setRides(ridesData);
      } catch (err) {
        setError(err.message);
      }
    };

    getDriversAndRides();
  }, []);

  const handleApproveDriver = async (username) => {
    try {
      const success = await approveDriver(username);
      if (success) {
        setDrivers(drivers.filter(driver => driver.username !== username));
      }
    } catch (err) {
      setError(err.message);
    }
  };

  const handleDeclineDriver = async (username) => {
    try {
      const success = await declineDriver(username);
      if (success) {
        setDrivers(drivers.filter(driver => driver.username !== username));
      }
    } catch (err) {
      setError(err.message);
    }
  };

  const handleBlockDriver = async (username) => {
    try {
      const success = await blockDriver(username);
      if (success) {
        setDrivers(drivers.map(driver => 
          driver.username === username ? { ...driver, blocked: true } : driver
        ));
      }
    } catch (err) {
      setError(err.message);
    }
  };

  const handleUnblockDriver = async (username) => {
    try {
      const success = await unblockDriver(username);
      if (success) {
        setDrivers(drivers.map(driver => 
          driver.username === username ? { ...driver, blocked: false } : driver
        ));
      }
    } catch (err) {
      setError(err.message);
    }
  };

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
              Username: {driver.username}, Email: {driver.email}
              <button onClick={() => handleApproveDriver(driver.username)}>Approve</button>
              <button onClick={() => handleDeclineDriver(driver.username)}>Decline</button>
              <button onClick={() => handleBlockDriver(driver.username)} disabled={driver.blocked}>Block</button>
              <button onClick={() => handleUnblockDriver(driver.username)} disabled={!driver.blocked}>Unblock</button>
            </li>
          ))}
        </ul>
      </div>
    </>
  );
};

export default Administrator;
