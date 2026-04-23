import { useEffect, useState } from "react";
import { getProducts } from "./productApi";

export const useProducts = (pageNumber) => {
  const [data, setDatas] = useState({
    items: [],
    totalPage: 1,
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await getProducts(pageNumber);
        setDatas(res.data);
      } catch (err) {
        console.error(err);
      }
    };

    fetchData();
  }, [pageNumber]);

  

  return {
    products: data.items,
    totalPage: data.totalPage,
  };
};