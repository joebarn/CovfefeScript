(module
      
      (func $add (param $p1 i32) (param $p2 i32) (result i32)
        get_local $p1
        get_local $p2
        i32.add
      )
      
      (export "add" (func $add))
      
    )