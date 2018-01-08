(module
      
	  (func $print (import "imports" "print") (param i32))
	  
	  (func $main (local $n i32) 

		i32.const 9
		i32.const 10
		i32.eq
		if  
			i32.const 1
			call $print
		else
			i32.const 0
			call $print
		end

		i32.const 0
		set_local $n

			loop 

				get_local $n
				i32.const 1
				i32.add
				set_local $n

				get_local $n
				i32.const 10
				i32.eq
					if
						br 2
					end

				get_local $n
				call $print

				;;br 0
			end

					
      )
	  
      (export "main" (func $main))
      
    )