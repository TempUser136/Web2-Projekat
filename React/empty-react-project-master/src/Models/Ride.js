
class RideModel {
    constructor(startAddress, destination, price, waitTime, status){
      this.startAddress = startAddress;
      this.destination = destination;
      this.price = price;
      this.waitTime=waitTime;
      this.status = status;
    }
  }
  
  export default RideModel;