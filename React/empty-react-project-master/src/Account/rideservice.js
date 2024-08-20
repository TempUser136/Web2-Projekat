import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL;

export const getUserRides = async (username, token) => {
  const response = await axios.get(`${API_URL}/ride/GetUserRides`, {
    params: { username },
    headers: { Authorization: `Bearer ${token}` },
  });
  return response.data;
};

export const addRide = async (rideData) => {
  const response = await axios.post(`${API_URL}/ride/AddRide`, rideData);
  return response.data;
};

export const calculateRide = async (startAddress, destination) => {
  const response = await axios.get(`${API_URL}/Ride/Calculate`, {
    params: {
      startAddress,
      destination,
    }
  });
  return response.data;
};
