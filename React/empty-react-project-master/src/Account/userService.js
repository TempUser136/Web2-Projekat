import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL;

export const getDrivers = async () => {
  const response = await axios.get(`${API_URL}/user/GetDrivers`);
  return response.data;
};

export const approveDriver = async (username) => {
  const response = await axios.post(`${API_URL}/user/ApproveDriver`, { username });
  return response.data;
};

export const declineDriver = async (username) => {
  const response = await axios.post(`${API_URL}/user/DeclineDriver`, { username });
  return response.data;
};

export const blockDriver = async (username) => {
  const response = await axios.post(`${API_URL}/user/BlockDriver`, { username });
  return response.data;
};

export const unblockDriver = async (username) => {
  const response = await axios.post(`${API_URL}/user/UnblockDriver`, { username });
  return response.data;
};
