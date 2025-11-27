package com.example.inspectxr.service;

import com.example.inspectxr.dtos.UserRequestDTO;
import com.example.inspectxr.dtos.UserResponseDTO;
import com.example.inspectxr.entity.UserEntity;
import com.example.inspectxr.mapper.UserMapper;
import com.example.inspectxr.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;

@Service
public class UserService {

    private final UserRepository userRepository;
    private final UserMapper userMapper;

    @Autowired
    public UserService(UserRepository userRepository, UserMapper userMapper) {
        this.userRepository = userRepository;
        this.userMapper = userMapper;
    }

    public UserResponseDTO register(UserRequestDTO dto) {
        if (userRepository.findByUsername(dto.getUsername()).isPresent()) {
            throw new ResponseStatusException(HttpStatus.CONFLICT, "Username already taken");
        }
        UserEntity user = userMapper.toEntity(dto);
        return userMapper.toDTO(userRepository.save(user));
    }

    public UserResponseDTO login(UserRequestDTO userRequestDTO) {
        return userRepository.findByUsername(userRequestDTO.getUsername())
                .filter(user -> user.getPassword().equals(userRequestDTO.getPassword()))
                .map(userMapper::toDTO)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid credentials"));
    }
}

