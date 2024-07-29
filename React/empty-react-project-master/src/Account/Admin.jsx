import React, { useEffect, useState } from 'react';
import axios from 'axios';

const Administrator = () => {
  const [drivers, setDrivers] = useState([]);
  const [error, setError] = useState(null);

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

    fetchDrivers();
  }, []);

  if (error) {
    return <div>{error}</div>;
  }

  return (
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
  );
};

export default Administrator;
