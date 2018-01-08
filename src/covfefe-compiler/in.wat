(module
      
	  (func $print (import "imports" "print") (param i32))
	  
      (func $add (param $p1 i32) (param $p2 i32) (result i32)
        get_local $p1
        get_local $p2
        i32.add
      )
      
	  (func $main 
      
	      i32.const 10 
	      i32.const 5
		  call $add
          call $print
      
      )
	  
      (export "main" (func $main))
      
    )