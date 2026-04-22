import { useEffect, useState } from "react";
import { getProducts } from "./productApi";

export const useProducts = () => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await getProducts();
        setProducts(res.data.items);
      } catch (err) {
        console.error(err);
      }
    };

    fetchData();
  }, []);

  return { products };
};