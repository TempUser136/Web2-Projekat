import React, { useEffect, useState } from 'react';
import { fetchUnapproved, fetchDrivers, fetchRides, approveDriver, declineDriver, blockDriver, unblockDriver } from '../api.js';
import "../Style/admin.css"
const Administrator = () => {
  const [unapprovedDrivers, setUnapprovedDrivers] = useState([]);
  const [approvedDrivers, setApprovedDrivers] = useState([]);
  const [error, setError] = useState(null);
  const [rides, setRides] = useState([]);

  useEffect(() => {
    const getDriversAndRides = async () => {
      try {
        // Fetch unapproved drivers
        const driversData = await fetchDrivers();
        setUnapprovedDrivers(driversData);

        // Fetch all drivers, including those already approved
        const allDrivers = await fetchUnapproved();
        // Separate the approved drivers
        setApprovedDrivers(allDrivers.filter(driver => !driversData.some(unapproved => unapproved.username === driver.username)));

        // Fetch rides
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
        const updatedDriver = unapprovedDrivers.find(driver => driver.username === username);
        setUnapprovedDrivers(unapprovedDrivers.filter(driver => driver.username !== username));
        setApprovedDrivers([...approvedDrivers, updatedDriver]);
      }
    } catch (err) {
      setError(err.message);
    }
  };

  const handleDeclineDriver = async (username) => {
    try {
      const success = await declineDriver(username);
      if (success) {
        setUnapprovedDrivers(unapprovedDrivers.filter(driver => driver.username !== username));
      }
    } catch (err) {
      setError(err.message);
    }
  };

  const handleBlockDriver = async (username) => {
    try {
      const success = await blockDriver(username);
      if (success) {
        setApprovedDrivers(approvedDrivers.map(driver => 
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
        setApprovedDrivers(approvedDrivers.map(driver => 
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
        <h1>Ride History</h1>
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
      
      <div>
        <h1>Unapproved Drivers</h1>
        <ul>
          {unapprovedDrivers.map((driver, index) => (
            <li key={index}>
              Username: {driver.username}, Email: {driver.email}
              <button onClick={() => handleApproveDriver(driver.username)}>Approve</button>
              <button onClick={() => handleDeclineDriver(driver.username)}>Decline</button>
            </li>
          ))}
        </ul>
      </div>

      <div>
        <h1>All users</h1>
        <ul>
          {approvedDrivers.map((driver, index) => (
            <li key={index}>
              Username: {driver.username}, Email: {driver.email}
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
