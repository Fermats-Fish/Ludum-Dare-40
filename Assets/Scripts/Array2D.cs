using System.Collections.Generic;

// 2D List which expands dynamically in both -ve and +ve directions.
public class Array2D<T> {

	// The array before it's been displaced to allow for -ve inputs.
	List<List<T>> array = new List<List<T>>();

	// The coordinates of the origin in the map.
	int originX = 0;
	int originY = 0;

	// What will be returned if you ask for a tile outside the map.
	T defaultItem = default(T);

	// Constructor.
	public Array2D (T defaultItem){
		this.defaultItem = defaultItem;

		// Add the origin to the map. 
		array.Add (new List<T> ());
		array [0].Add ((defaultItem));
	}

	// Returns the smallest x coord.
	public int GetMinX(){
		return -originX;
	}

	// Returns the largest x coord.
	public int GetMaxX(){
		return array.Count - originX - 1;
	}

	// Returns the smallest y coord.
	public int GetMinY(){
		return -originY;
	}

	// Returns the largest y coord.
	public int GetMaxY(){
		return array [0].Count - originX - 1;
	}

	public T GetAt(int x, int y){
		// Check if it is within the bounds of the array.
		if ( GetMinX() < x  &&  x < GetMaxX()  &&  GetMinY() < y  &&  y < GetMaxY() ){

			// It is so return the tile at those coords.
			return array [x + originX] [y + originY];

		} else {

			// It isn't so return the default one.
			return defaultItem;

		}
	}

	public void setAt(int x, int y, T item){
		// If the coordinate being set are out of the map, expand the map until it fits.

		while (x < GetMinX()){
			// Need to exapnd the array to the left.

			// Add a new column.
			array.Insert (0, new List<T>());

			// Now populate the column to the right size.
			while (array[0].Count < array[1].Count){
				array [0].Add (defaultItem);
			}

			// Finally the coords of the origin have moved to the right.
			originX += 1;
		}

		while (x > GetMaxX()){
			// Need to expand the array to the right.

			// Add a new column.
			array.Add (new List<T>());

			// Now populate the column to the right size.
			while (array[array.Count-1].Count < array[0].Count){
				array [array.Count-1].Add (defaultItem);
			}
		}

		while (y < GetMinY()){
			// Need to expand the array in the -ve y direction.

			// Add a new T at the start of each column.
			for (int i = 0; i < array.Count; i++) {
				array [i].Insert (0, defaultItem);
			}

			// Finally the coords of the origin have moved.
			originY += 1;
		}

		while (y > GetMaxY()){
			// Need to expand the array in the +ve y direction.

			// Add a new T at the end of each column.
			for (int i = 0; i < array.Count; i++) {
				array [i].Add (defaultItem);
			}
		}

		// Now finally set the item at those coords.
		array [x + originX][y + originY] = item;
	}

}